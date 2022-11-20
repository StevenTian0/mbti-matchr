import socketserver
import sys
import math
import time
import logging

TCP_HOST = "0.0.0.0"
TCP_PORT = 51234

client_1_data = bytearray()
client_2_data = bytearray()


class TCPHandler(socketserver.BaseRequestHandler):
    """
    Instantiated once per connection
    """

    def handle(self):
        logging.info(f"TCP client connected: {self.client_address[0]}")
        while True:
            data = self.request.recv(1024).strip()
            if data == b"BUGZAPPR":
                greeting = "Hello world"
                message = bytearray(f"{greeting}", encoding="utf-8")
                self.request.sendall(message)
            else:  # b''
                logging.info(f"TCP client disconnected: {self.client_address[0]}")
                break


if __name__ == "__main__":
    # set up log to console
    logger = logging.getLogger()
    logger.setLevel(logging.INFO)
    formatter = logging.Formatter("[%(asctime)s.%(msecs)03d] [%(levelname)s] %(message)s", "%Y-%m-%d %H:%M:%S")
    stdout_handler = logging.StreamHandler(sys.stdout)
    stdout_handler.setFormatter(formatter)
    logger.addHandler(stdout_handler)
    # start server
    with socketserver.TCPServer((TCP_HOST, TCP_PORT), TCPHandler) as server:
        logging.info("Started TCP server")
        server.serve_forever()
