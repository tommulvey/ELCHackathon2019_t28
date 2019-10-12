from __future__ import absolute_import, print_function
import json, base64
from tweepy import OAuthHandler, Stream, StreamListener
from azure.storage.queue import QueueService

# Go to http://apps.twitter.com and create an app.
# The consumer key and secret will be generated for you after
consumer_key='hxTeaYaXrtyeDwgCoahvmjEpX'
consumer_secret='h1XZeSY8sgMmuhaEXPmBLbIeazEDsgXUtAEvUks0lyKiRPN3kw'

# After the step above, you will be redirected to your app's page.
# Create an access token under the the "Your access token" section
access_token='1182794983247618048-Ojc2r9RQxN4lKTuSP617bEVjT4HGwF'
access_token_secret='t18MLzH560E66PvNAG97cPJfIdyseGtsNi2d2VmCLyEOf'
queue_service = QueueService(account_name='elcqueues2019', account_key='BGyZDgWXKYEQUF31pvLVfk3b9EZMhiCPS7MUjnZgGfQKZ9Lthd6BwK3ITfE27ROdRU/zZGAkZkRsBLxPRn4U5g==')
class StdOutListener(StreamListener):
    """ A listener handles tweets that are received from the stream.
    This is a basic listener that just prints received tweets to stdout.
    """
    def on_data(self, data):
        # print(data)
        #json_acceptable_string = data.replace("'", "\"")
        d = json.loads(data)

        if "retweeted_status" not in d or str(d["retweeted_status"]) is "false":
            link = "https://twitter.com/"+d["user"]["screen_name"]+"/status/"+str(d["id"])
            date = str(d["created_at"])
            like = str(d["favorite_count"])
            rts = str(d["retweet_count"])
            body = d["text"]
            name = d["user"]["name"]
            id1 = str(d["id"])
            profile = d["user"]["profile_image_url"]
            postDict = {
                'date': date,
                'likes': like,
                'rts': rts,
                'body': body,
                'name': name,
                'profile': profile,
                'link': link,
                'id': id1 
            }
            app_json = json.dumps(postDict, sort_keys=True)
            s = str(base64.b64encode(app_json.encode('utf-8')))
            s = s[2:-1]
            queue_service.put_message('tweets', s)
            print(str(s))

        return True

    def on_error(self, status):
        print(status)

if __name__ == '__main__':
    l = StdOutListener()
    auth = OAuthHandler(consumer_key, consumer_secret)
    auth.set_access_token(access_token, access_token_secret)

    stream = Stream(auth, l)
    stream.filter(track=['Blue Heart', 'blue heart', 'blueheart', 'Blue heart', 'Blueheart'])
    

