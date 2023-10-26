from locust import HttpUser, task


class IndexUser(HttpUser):
    @task
    def ping(self):
        self.client.get("/index")
