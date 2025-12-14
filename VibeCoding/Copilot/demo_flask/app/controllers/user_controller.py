from flask import Blueprint, jsonify

user_bp = Blueprint("user", __name__, url_prefix="/user")


@user_bp.route("", methods=["GET"])
def users():
    return {"users": [{"id": 1, "name": "Ada"}, {"id": 2, "name": "Linus"}]}, 201
