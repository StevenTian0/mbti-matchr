import socket

sock = socket.create_connection(("127.0.0.1", 51234))
sock.sendall(b"MAGIC")
received_1 = sock.recv(1024)
print(received_1)

