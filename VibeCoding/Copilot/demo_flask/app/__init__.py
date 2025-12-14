from flask import Flask
from app.controllers.health_controller import health_bp
from app.controllers.user_controller import user_bp


def create_app():
    app = Flask(__name__)
    app.register_blueprint(health_bp)
    app.register_blueprint(user_bp)

    # 新增首頁路由
    @app.get("/")
    def index():
        return "Flask app is running. Try /health or /users."

    return app
