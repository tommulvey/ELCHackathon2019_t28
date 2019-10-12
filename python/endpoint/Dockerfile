FROM python:3.6.4

RUN apt-get update -y && \
    apt-get install -y python-pip python-dev

# We copy just the requirements.txt first to leverage Docker cache
COPY ./requirements.txt /app/requirements.txt

WORKDIR /app
RUN pip install azure-cosmos==4.0.0b2
RUN pip install -r requirements.txt 

COPY . /app

ENTRYPOINT [ "python" ]

CMD [ "endpoint.py" ]