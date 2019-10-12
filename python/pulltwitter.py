try:
    import json
except ImportError:
    import simplejson as json
import base64

from azure.storage.queue import QueueService
import tweepy

#credentials
ACCESS_TOKEN = '1182794983247618048-Ojc2r9RQxN4lKTuSP617bEVjT4HGwF'
ACCESS_SECRET = 't18MLzH560E66PvNAG97cPJfIdyseGtsNi2d2VmCLyEOf'
CONSUMER_KEY = 'hxTeaYaXrtyeDwgCoahvmjEpX'
CONSUMER_SECRET = 'h1XZeSY8sgMmuhaEXPmBLbIeazEDsgXUtAEvUks0lyKiRPN3kw'

# Use tweepy to connect to twitter with credentials

auth = tweepy.OAuthHandler(CONSUMER_KEY, CONSUMER_SECRET)
auth.set_access_token(ACCESS_TOKEN, ACCESS_SECRET)
api = tweepy.API(auth, wait_on_rate_limit=True, wait_on_rate_limit_notify=True, compression=True)
public_tweets = api.home_timeline()

queue_service = QueueService(account_name='elcqueues2019', account_key='BGyZDgWXKYEQUF31pvLVfk3b9EZMhiCPS7MUjnZgGfQKZ9Lthd6BwK3ITfE27ROdRU/zZGAkZkRsBLxPRn4U5g==')
print("nice")
for tweet in api.search(q="blue ocean -filter:retweets", lang="en", rpp=10):
    link = f"https://twitter.com/{tweet.user.screen_name}/status/{tweet.id}"
    date = (f"{tweet.created_at}")
    like = (f"{tweet.favorite_count}")
    rts = (f"{tweet.retweet_count}")
    body = (f"{tweet.text}")
    name = (f"{tweet.user.name}")
    id = (f"{tweet.id}")
    profile = (f"{tweet.user.profile_image_url}")
    postDict = {
        'date': date,
        'likes': like,
        'rts': rts,
        'body': body,
        'name': name,
        'profile': profile,
        'link': link,
        'id': id 
    }
    app_json = json.dumps(postDict, sort_keys=True)
    s = str(base64.b64encode(app_json.encode('utf-8')))
    s = s[2:-1]
    queue_service.put_message('tweets', s)
    print(str((f"{tweet.id}")))
    #print((f"{tweet.user.name}:{tweet.text}").encode("utf-8"))

#for tweet in tweepy.Cursor(api.search,
#                           q = "save ocean",
#                           until = "2019-10-12",
#                           lang = "en", rpp=5).items():
#    postDict = {
#        'date': 'tech',
#        'federer': 'tennis',
#        'ronaldo': 'football',
#        'woods': 'golf',
#        'ali': 'boxing'
#    }
