import json
import faiss
import numpy as np
from sentence_transformers import SentenceTransformer

def load_data():
    with open('database/courses.json', 'r', encoding='utf-8') as file:
        courses_data = json.load(file)
    return courses_data

def create_faiss_index(courses_data, model_name='sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2'):
    print("Loading model...")
    model = SentenceTransformer(model_name)
    print("Model loaded.")
    
    index = faiss.IndexFlatL2(model.get_sentence_embedding_dimension())
    print("Index created.")
    
    course_ids = []
    embeddings = []

    for course_id, course in courses_data.items():
        text = course.get('course_name', '') + ' ' + course.get('course_type', '') + ' ' + course.get('suitable_for_professions', '') + ' ' + ' '.join(course.get('tags', []))
        embedding = model.encode(text)
        embeddings.append(embedding)
        course_ids.append(course_id)
    
    embeddings = np.array(embeddings).astype('float32')
    index.add(embeddings)
    print("Embeddings added to index.")
    
    faiss.write_index(index, 'database/faiss_index.bin')
    print("Index written to file in /app/database/faiss_index.bin.")
    
    with open('database/courses.json', 'w', encoding='utf-8') as f:
        json.dump({"courses_data": courses_data, "course_ids": course_ids}, f, ensure_ascii=False, indent=4)
    print("course_ids added to courses.json in /app/database/")

if __name__ == "__main__":
    print("Loading data...")
    courses_data = load_data()
    print("Data loaded.")
    create_faiss_index(courses_data)
    print("FAISS index created.")
