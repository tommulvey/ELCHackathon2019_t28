try:
    import json
except ImportError:
    import simplejson as json

tweets_filename = 'twitter.txt'
tweets_file = open(tweets_filename, "r")


for line in tweets_file:
    try:

        tweet = json.loads(line.strip())
        if 'text' in tweet:
            print(tweet['id'])
            print(tweet['created_at'])
            print(tweet['text'])

            print(tweet['user']['id'])
            print(tweet['user']['name'])
            print(tweet['user']['screen_name'])

            hashtags = []
            for hashtag in tweet['entities']['hashtags']:
                hashtags.append(hashtag['text'])
            print(hashtags)

    except:
        print('except')
        continue
