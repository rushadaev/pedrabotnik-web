# Педработник Рекомандации 

Этот проект предоставляет API для генерации рекомендаций по продуктам с использованием предварительно обученной модели.

## Важно 
**Для работы проекта необходимо наличие модели в папке `models/paraphrase-multilingual-MiniLM-L12-v2`** 

## Требования
- Docker

## Установка

### Шаг 1: Установка переменных окружения
Создайте файл `.env` в корне директории проекта и добавьте ваш ключ API OpenAI:

**.env**:
```plaintext
OPENAI_API_KEY=your_openai_api_key_here
```

### Шаг 2: Загрузка файлов модели
Необходимо вручную загрузить файлы модели и поместить их в директорию `models`.
1. Скачайте файлы модели с [Hugging Face](https://huggingface.co/sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2), либо используйте zip архив с моделью
2. Поместите скачанные файлы в директорию `models/paraphrase-multilingual-MiniLM-L12-v2`.

Директория должна выглядеть следующим образом:

```
models
└── paraphrase-multilingual-MiniLM-L12-v2
    ├── config.json
    ├── config_sentence_transformers.json
    ├── model.safetensors
    ├── modules.json
    ├── pytorch_model.bin
    ├── sentence_bert_config.json
    ├── sentencepiece.bpe.model
    ├── special_tokens_map.json
    ├── tf_model.h5
    ├── tokenizer.json
    ├── tokenizer_config.json
    └── unigram.json
```
### Шаг 3: Запуск проекта 
   ```sh
   make start 
   ```

## API эндпоинты 

- `POST http://localhost:5001/recommendations`: Получить рекомендации по запросу. 

### Пример запроса
Пример запроса к эндпоинту `/recommendations` с использованием `curl`:

В запросе необходимо передать JSON объект с ключом `query` и значением в виде строки с запросом.

```sh
curl -X POST http://localhost:5001/recommendations -H "Content-Type: application/json" -d '{"query": "Какая длина тренингов?"}'
```

### Пример ответа 
```json
[
    {
        "answer": "36 часов - 5 дней\n72 часа - 9 дней\n108 часов - 14 дней\n144 часа - 18 дней",
        "question": "какой срок обучения?"
    },
    {
        "answer": "288 часов - 36 дней\n502 часа - 63 дня\n650 часов - 82 дня\n1020 часов - 128 дней",
        "question": "сколько длится курс переподготовки?"
    },
    {
        "professional_retraining": "от 250 часов до 1020 часов",
        "qualification_increase": "от 16 часов до 144 часов"
    }
]
```