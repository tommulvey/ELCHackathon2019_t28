FROM python:3.7

# RUN apt-get update -y && \
#     apt-get install -y python-pip python-dev

# We copy just the requirements.txt first to leverage Docker cache
# COPY ./requirements.txt /app/requirements.txt

WORKDIR /app
RUN pip install azure-cosmos==4.0.0b2
RUN pip install tweepy
# RUN pip install json
# RUN pip install base64
# RUN pip install -r requirements.txt 

COPY . /app

ENTRYPOINT [ "python" ]

CMD [ "tweepylive.py" ]