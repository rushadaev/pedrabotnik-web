import json
import faiss
import numpy as np
import gc
import os
from dotenv import load_dotenv
from openai import OpenAI
from sentence_transformers import SentenceTransformer, models

load_dotenv()
api_key = os.getenv('OPENAI_API_KEY')

client = OpenAI(api_key=api_key)

def get_chatgpt_response(query):
    simplified_structure = {
        "organization":
        ["name", "contact_phone", "website", "address", "email"],
        "general_info": [
            "description", "programs", "individual_approach", "recognition",
            "details"
        ],
        "legal_info": [
            "license", "status", "registration", "students", "publications",
            "services"
        ],
        "advantages": [
            "documents", "security", "individual_approach", "format",
            "free_services", "knowledge"
        ],
        "steps_for_learning": [],
        "program_details": [
            "additional_professional_education", "form_of_study", "language",
            "duration", "license_number"
        ],
        "course_advantages": [
            "documents", "security", "individual_approach", "format",
            "free_services", "knowledge"
        ],
        "steps_for_distance_learning": [],
        "video_instructions": ["individual", "organization"],
        "reviews": ["overview", "individual_reviews"],
        "training_areas": [
            "preschool_education", "primary_education", "secondary_education",
            "professional_education", "additional_education",
            "administrative_staff", "driving_school"
        ],
        "hours_of_training":
        ["qualification_increase", "professional_retraining"],
        "learning_process": ["correspondence", "costs"],
        "benefits": [
            "consultation", "professional_retraining", "remote_technology",
            "payment_plan"
        ],
        "faq": [
            "state_institution", "document", "government_document",
            "distance_learning_document", "required_documents", "start_time",
            "requirements_for_learning", "distance_learning_process",
            "final_certification", "visit_for_certification",
            "document_delivery", "payment_methods", "installment_plan",
            "refund_policy", "professional_retraining_benefits",
            "retraining_vs_second_degree", "qualification_in_diploma",
            "retraining_without_pedagogical_education",
            "qualification_increase_benefits", "basic_education_for_courses",
            "education_for_school_graduates", "retraining_with_npo_diploma",
            "document_not_received"
        ],
        "courses": [
            "course_name", "course_type", "course_page_link",
            "pricing_and_course_length", "suitable_for_professions",
            "syllabus", "tags"
        ]
    }

    prompt_message = f"""
    Please categorize the following query based on the structure

    Simplified Structure:
    {json.dumps(simplified_structure, indent=2)}

    Query: {query}

    Example
    user: Какие рекомендации по курсу фгос?
    json response: {{'category':'courses', 'subcategory':'course_name', 'query': 'фгос'}}
    """

    response = client.chat.completions.create(
        model="gpt-4-turbo",
        response_format={"type": "json_object"},
        messages=[{
            "role":
            "system",
            "content":
            "You are a categorizer. Please respond in JSON format."
        }, {
            "role": "user",
            "content": prompt_message
        }])
    return json.loads(response.choices[0].message.content.strip())


def load_data():
    with open('database/courses.json', 'r', encoding='utf-8') as file:
        data = json.load(file)
    courses_data = data['courses_data']
    course_ids = data['course_ids']
    with open('database/common.json', 'r', encoding='utf-8') as file:
        common_data = json.load(file)
    return courses_data, course_ids, common_data


def load_faiss_index():
    index = faiss.read_index('database/faiss_index.bin')
    return index


def load_faiss_qa_index():
    index = faiss.read_index('database/faiss_qa_index.bin')
    return index


def search_qa(query, index, model_name='/app/models/paraphrase-multilingual-MiniLM-L12-v2'):
    # Ensure the model path is absolute
    model_path = os.path.abspath(model_name)
    # Load the model using the absolute path
    word_embedding_model = models.Transformer(model_path)
    pooling_model = models.Pooling(word_embedding_model.get_word_embedding_dimension())
    model = SentenceTransformer(modules=[word_embedding_model, pooling_model])
    
    query_embedding = model.encode(query).astype('float32')
    D, I = index.search(np.array([query_embedding]), k=2)
    return I[0]


def search_courses(
        query,
        model_name='/app/models/paraphrase-multilingual-MiniLM-L12-v2'
):
    model_path = os.path.abspath(model_name)
    # Load the model using the absolute path
    word_embedding_model = models.Transformer(model_path)
    pooling_model = models.Pooling(word_embedding_model.get_word_embedding_dimension())
    model = SentenceTransformer(modules=[word_embedding_model, pooling_model])
    
    query_embedding = model.encode(query).astype('float32')
    index = load_faiss_index()
    D, I = index.search(np.array([query_embedding]), k=5)
    return I[0]


def find_in_courses(data, course_ids, subcategory, query=None):
    results = []
    if query:
        top_indices = search_courses(query)
        top_course_ids = [course_ids[i] for i in top_indices]
        data = {k: v for k, v in data.items() if k in top_course_ids}

    for item in data.values():
        if subcategory in item:
            filtered_item = {k: v for k, v in item.items() if k not in ['syllabus', 'tags']}
            if subcategory in filtered_item:
                results.append(filtered_item)

    return results


def calculate_average_pricing(pricing_dict):
    total_price = 0
    total_hours = 0
    count = 0

    for hours, price in pricing_dict.items():
        try:
            hours_value = int(''.join(filter(str.isdigit, hours)))
            price_value = int(''.join(filter(str.isdigit, price)))
            total_hours += hours_value
            total_price += price_value
            count += 1
        except ValueError:
            continue

    if count == 0:
        return "Invalid data for pricing calculation"

    average_price = total_price / count
    average_hours = total_hours / count
    return f"Средняя цена: {average_price} рублей, Среднее количество часов: {average_hours}"


def display_column(data, category, subcategory):
    if subcategory:
        if subcategory in data.get(category, {}):
            return data[category][subcategory]
        else:
            return f"{subcategory} not found in {category}"
    else:
        if category in data:
            return data[category]
        else:
            return f"{category} not found"


def load_qa_data():
    with open('database/q&a.json', 'r', encoding='utf-8') as file:
        data = json.load(file)
    return data['dialogs']


def get_product_recommendations(query):
    courses_data, course_ids, common_data = load_data()
    qa_index = load_faiss_qa_index()
    dialogs = load_qa_data()

    category_response = get_chatgpt_response(query)
    category = category_response.get('category', '').lower() if category_response.get('category') else ''
    subcategory = category_response.get('subcategory', '').lower() if category_response.get('subcategory') else ''
    query_text = category_response.get('query', '').lower() if category_response.get('query') else ''

    if category == 'courses':
        qa_indices = search_qa(query, qa_index)
        results_faq = [dialogs[i] for i in qa_indices]
        results_common = find_in_courses(courses_data, course_ids, subcategory, query_text)
        results = results_faq + results_common

    elif category == 'faq':
        qa_indices = search_qa(query_text, qa_index)
        results_faq = [dialogs[i] for i in qa_indices]
        results_common = display_column(common_data, category, subcategory)
        if not isinstance(results_common, list):
            results_common = [results_common]
        results = results_faq + results_common
    else:
        qa_indices = search_qa(query, qa_index)
        results_faq = [dialogs[i] for i in qa_indices]
        results_common = display_column(common_data, category, subcategory)
        if not isinstance(results_common, list):
            results_common = [results_common]
        results = results_faq + results_common

    # Call garbage collector to free up memory
    gc.collect()

    return results
