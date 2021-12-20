import time
import os
from subprocess import Popen, PIPE

p = Popen([os.getcwd() + "\SnakeAI.exe", "ai"], stdin = PIPE, stdout = PIPE, shell = True)
i = 0
while True:
    j = 0
    for j in range(20):
        print(p.stdout.readline())
    p.stdout.flush()
    if i < 2:
        p.stdin.write("w\n".encode())
    elif i < 4:
        p.stdin.write("a\n".encode())
    elif i < 6:
        p.stdin.write("s\n".encode())
    elif i < 8:
        p.stdin.write("d\n".encode())
    p.stdin.flush()
    i += 1
    if i == 8:
        i = 0
    time.sleep(1)
    os.system("cls")