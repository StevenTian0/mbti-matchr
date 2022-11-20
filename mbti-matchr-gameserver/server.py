import socketserver
import sys
import math
import time
import logging

TCP_HOST = "0.0.0.0"
TCP_PORT = 51234


class TCPHandler(socketserver.BaseRequestHandler):
    """
    Instantiated once per connection
    """

    def handle(self):
        global player_1_ready, player_2_ready, player_1_data, player_2_data
        logging.info(f"TCP client connected: {self.client_address[0]}")
        while True:
            data = self.request.recv(2048).strip()
            # ready
            if data[0:7] == b"BZREADY" and len(data) == 8:
                pid = int(bytearray(data[7:8]).decode("utf-8"))
                ready = 0
                # first player call
                if pid == 0:
                    if not player_1_ready:
                        player_1_ready = True
                        pid = 1
                        logging.info(f"TCP client {self.client_address[0]} is assigned to P1")
                    elif not player_2_ready:
                        player_2_ready = True
                        pid = 2
                        ready = 1
                        logging.info(f"TCP client {self.client_address[0]} is assigned to P2")
                else:
                    if player_1_ready and player_2_ready:
                        ready = 1
                message = bytearray(f"{pid},{ready}", encoding="utf-8")
                self.request.sendall(message)
            elif data[0:8] == b"BZUPDATE" and len(data) > 9:
                pid = int(bytearray(data[8:9]).decode("utf-8"))
                logging.info(f"P{pid} requested update")
                message = bytearray(b"{}")
                if pid == 1:
                    logging.info(f"Updating P{pid} data")
                    player_1_data = bytearray(data[9:])
                    logging.info(f"Setting P{pid} message")
                    message = player_2_data
                elif pid == 2:
                    logging.info(f"Updating P{pid} data")
                    player_2_data = bytearray(data[9:])
                    logging.info(f"Setting P{pid} message")
                    message = player_1_data
                logging.info(f"Sending message to P{pid}")
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
    # constants
    player_1_ready = False
    player_2_ready = False
    player_1_data = bytearray(b"{}")
    player_2_data = bytearray(b"{}")
    # start server
    with socketserver.TCPServer((TCP_HOST, TCP_PORT), TCPHandler) as server:
        logging.info("Started TCP server")
        server.serve_forever()
