from flask import Flask, request, jsonify
import subprocess
import os
from utils import get_product_recommendations

app = Flask(__name__)

@app.route('/recommendations', methods=['POST'])
def recommendations():
    data = request.json
    query = data.get('query')
    recommendations = get_product_recommendations(query)
    return jsonify(recommendations)

@app.route('/updateindexes', methods=['POST'])
def update_indexes():
    try:
        subprocess.run(['python', 'create_faiss_qa_index.py'], check=True)
        subprocess.run(['python', 'create_faiss_index.py'], check=True)
        return jsonify({"status": "Indexes updated successfully"}), 200
    except subprocess.CalledProcessError as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
