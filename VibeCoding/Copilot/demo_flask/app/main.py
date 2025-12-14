from . import create_app

if __name__ == "__main__":
    create_app().run("0.0.0.0",debug=True,port=5000,use_reloader=True)
