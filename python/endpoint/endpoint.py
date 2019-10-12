try:
    import json
except ImportError:
    import simplejson as json
from datetime import datetime
import time
from flask import Flask, jsonify
from flask import request
import azure.cosmos.documents as documents
import azure.cosmos.cosmos_client as cosmos_client
import azure.cosmos.errors as errors

url = 'https://team28.documents.azure.com/'
key = 'lVLk1cd6W79qgTbp7DAfs9QoLxFuewOsgVYLrXRHP9QOufaVdbv3IH274fa9gDYKpyoYBg3RlTF3bLjWvTq2TA=='
client = cosmos_client.CosmosClient(url, {'masterKey': key} )
database = client.get_database_client('tweets')
container = database.get_container_client('tweets')

app = Flask(__name__)
@app.route('/api/cdate', methods=['GET'])
def get_time():
    s = '{'
    for item in container.query_items(
                    query='SELECT tweets.date FROM tweets where tweets.date >= "2019-10-01" and tweets.date <= "2019-11-01"',
                    enable_cross_partition_query=True):
        jtime = item['date']
        head, text, end = jtime.partition('T')
        rtime = datetime.strptime(head, '%Y-%m-%d').timetuple()
        unixt = int(time.mktime(rtime))
        s = s + str(unixt) + ':' + str(1) + ','
    s = s[:-1]
    s = s + '}'
    res = jsonify(s)
    res.headers.add('Access-Control-Allow-Origin', '*')
    return res

@app.route('/api/stats', methods=['GET'])
def stats():
    month = str(request.args.get('months', default = "10", type = str))
    day = str(request.args.get('days', default = "*", type = str))
    if (day=="*" and month=="") or (month=="*" and day=="*") or (int(month) > 12 or int(month) < 1):
        return "bad"
    # scenario 1. all months/days need to be calculated. return
    if (month!="*" and day=="*"):

        # num posts
        res = {}
        for item in container.query_items(
            query='SELECT VALUE COUNT(1) FROM tweets where tweets.date >= "2019-'+str(month)+'-01" and tweets.date < "2019-'+str(int(month)+1)+'-01"',
            enable_cross_partition_query=True
            ):
            res["posts"]=str(item)
        #likes
        for item in container.query_items(
            query='SELECT VALUE SUM(tweets.likes) FROM tweets where tweets.date >= "2019-'+str(month)+'-01" and tweets.date < "2019-'+str(int(month)+1)+'-01"',
            enable_cross_partition_query=True
            ):
            res["likes"]=str(item)
        #rts
        for item in container.query_items(
            query='SELECT VALUE SUM(tweets.rts) FROM tweets where tweets.date >= "2019-'+str(month)+'-01" and tweets.date < "2019-'+str(int(month)+1)+'-01"',
            enable_cross_partition_query=True
            ):
            res["rts"]=str(item)
        
        return res


if __name__ == '__main__':
    app.jinja_env.auto_reload = True
    app.config['TEMPLATES_AUTO_RELOAD'] = True
    app.run(host='0.0.0.0', debug=True, port=80)
