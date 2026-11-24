"""Usage reporting service: monthly usage, support event lookup and snapshot import."""
import json
import pickle
import sqlite3
from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import parse_qs, urlparse

PORT = 5000
DB_PATH = "reporting.db"

# Event types the support console is allowed to search for.
ALLOWED_EVENT_TYPES = {"invoice.created", "invoice.paid", "invoice.payment_failed"}

# Report snapshots cached in memory as pickled bytes, created only by this process.
_snapshot_cache = {}


def get_connection():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def init_schema(conn):
    conn.executescript(
        """
        CREATE TABLE IF NOT EXISTS usage (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            customer_id TEXT NOT NULL,
            month TEXT NOT NULL,
            api_calls INTEGER NOT NULL DEFAULT 0,
            invoices_issued INTEGER NOT NULL DEFAULT 0
        );
        CREATE TABLE IF NOT EXISTS events (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            customer_id TEXT NOT NULL,
            event_type TEXT NOT NULL,
            created_at TEXT NOT NULL
        );
        """
    )
    conn.commit()


def get_usage_for_customer(conn, customer_id, month):
    """Monthly usage for one customer (parameterized query)."""
    cur = conn.execute(
        "SELECT customer_id, month, api_calls, invoices_issued "
        "FROM usage WHERE customer_id = ? AND month = ?",
        (customer_id, month),
    )
    row = cur.fetchone()
    return dict(row) if row else None


def search_events(conn, event_type):
    """Support console lookup of recent events by type."""
    if event_type not in ALLOWED_EVENT_TYPES:
        raise ValueError("unsupported event type")

    # Scanner test: string-built SQL. Not exploitable, because event_type can only be
    # one of the fixed values in ALLOWED_EVENT_TYPES (checked above).
    query = (
        "SELECT customer_id, event_type, created_at FROM events "
        f"WHERE event_type = '{event_type}' ORDER BY created_at DESC LIMIT 100"
    )
    return [dict(r) for r in conn.execute(query).fetchall()]


def save_snapshot(name, snapshot):
    _snapshot_cache[name] = pickle.dumps(snapshot)


def load_snapshot(name):
    # Scanner test: pickle.loads. Not exploitable, because the bytes were produced by
    # save_snapshot in this same process and never come from a request or a file.
    return pickle.loads(_snapshot_cache[name])


def summarize(snapshot):
    """Totals for a report snapshot (a dict of customer_id -> usage dict)."""
    return {
        "customers": len(snapshot),
        "api_calls": sum(u.get("api_calls", 0) for u in snapshot.values()),
        "invoices_issued": sum(u.get("invoices_issued", 0) for u in snapshot.values()),
    }


class Handler(BaseHTTPRequestHandler):
    def _send(self, status, payload):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def do_GET(self):
        url = urlparse(self.path)
        params = parse_qs(url.query)
        conn = get_connection()
        try:
            if url.path == "/healthz":
                self._send(200, {"status": "ok"})
            elif url.path == "/usage":
                usage = get_usage_for_customer(
                    conn, params.get("customer_id", [""])[0], params.get("month", [""])[0]
                )
                self._send(200 if usage else 404, usage or {"error": "not found"})
            elif url.path == "/events":
                try:
                    self._send(200, search_events(conn, params.get("type", [""])[0]))
                except ValueError as err:
                    self._send(400, {"error": str(err)})
            else:
                self._send(404, {"error": "not found"})
        finally:
            conn.close()

    def do_POST(self):
        if urlparse(self.path).path != "/reports/import":
            self._send(404, {"error": "not found"})
            return

        length = int(self.headers.get("Content-Length", 0))
        snapshot = json.loads(self.rfile.read(length))
        if not isinstance(snapshot, dict):
            self._send(400, {"error": "snapshot must be a JSON object"})
            return

        save_snapshot("latest", snapshot)
        self._send(200, summarize(load_snapshot("latest")))


def main():
    conn = get_connection()
    init_schema(conn)
    conn.close()
    server = HTTPServer(("0.0.0.0", PORT), Handler)
    print(f"reporting service listening on {PORT}")
    server.serve_forever()


if __name__ == "__main__":
    main()