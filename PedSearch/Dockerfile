FROM python:3.12-slim

# Install dependencies
RUN apt-get update && apt-get install -y \
    build-essential \
    && rm -rf /var/lib/apt/lists/*

# Set up virtual environment
RUN python -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

# Install Python packages
RUN pip install faiss-cpu sentence-transformers numpy flask openai gunicorn python-dotenv

# Copy application files
COPY utils.py /app/
COPY app.py /app/
COPY create_faiss_qa_index.py /app/
COPY create_faiss_index.py /app/
COPY database /app/database/
COPY models /app/models
COPY .env /app/

WORKDIR /app

# Run the Flask application with Gunicorn
CMD ["gunicorn", "-w", "4", "-b", "0.0.0.0:5000", "app:app"]
