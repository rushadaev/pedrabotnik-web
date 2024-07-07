import json
import faiss
import numpy as np
from sentence_transformers import SentenceTransformer

def load_qa_data():
    with open('database/q&a.json', 'r', encoding='utf-8') as file:
        data = json.load(file)
    return data['dialogs']

def encode_questions(dialogs, model_name='sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2'):
    model = SentenceTransformer(model_name)
    questions = [dialog['question'] for dialog in dialogs]
    question_embeddings = model.encode(questions).astype('float32')
    return question_embeddings

def save_faiss_index(question_embeddings, index_file='database/faiss_qa_index.bin'):
    index = faiss.IndexFlatL2(question_embeddings.shape[1])
    index.add(question_embeddings)
    faiss.write_index(index, index_file)

def main():
    dialogs = load_qa_data()
    question_embeddings = encode_questions(dialogs)
    save_faiss_index(question_embeddings)

if __name__ == "__main__":
    main()