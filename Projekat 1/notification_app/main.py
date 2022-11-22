from cgitb import text
from concurrent import futures
import time

import grpc
import greet_pb2
import greet_pb2_grpc


class NotificationServicer(greet_pb2_grpc.GreeterServicer):
    def Notify(self, request, context):
        #string1 = request.Title
        #string2 = string1.a.replace('Distributor', 'Currency')
        print("Must convert currency " + request.Title + "!")
        print(request)
        return greet_pb2.Reply(text="Got it right")


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    greet_pb2_grpc.add_GreeterServicer_to_server(
        NotificationServicer(), server)
    server.add_insecure_port("[::]:5011")
    server.start()
    print("Grpc server started on port 5011")
    server.wait_for_termination()


if __name__ == "__main__":
    serve()
