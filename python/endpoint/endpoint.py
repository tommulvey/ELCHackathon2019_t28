try:
    import json
except ImportError:
    import simplejson as json
from datetime import datetime
import time
from flask import Flask, jsonify
import azure.cosmos.documents as documents
import azure.cosmos.cosmos_client as cosmos_client
import azure.cosmos.errors as errors
import collections, functools, operator
from ast import literal_eval

url = 'https://team28.documents.azure.com/'
key = 'lVLk1cd6W79qgTbp7DAfs9QoLxFuewOsgVYLrXRHP9QOufaVdbv3IH274fa9gDYKpyoYBg3RlTF3bLjWvTq2TA=='
client = cosmos_client.CosmosClient(url, {'masterKey': key} )
database = client.get_database_client('tweets')
container = database.get_container_client('tweets')

app = Flask(__name__)
@app.route('/api/cdate', methods=['GET'])
def get_time():
    s = {}
    for item in container.query_items(
                    query='SELECT tweets.date FROM tweets where tweets.date >= "2019-10-01" and tweets.date <= "2019-11-01"',
                    enable_cross_partition_query=True):
        jtime = item['date']
        head, text, end = jtime.partition('T')
        rtime = datetime.strptime(head, '%Y-%m-%d').timetuple()
        unixt = int(time.mktime(rtime))
        strt = str(unixt)
        if strt in s :
            s[strt] = s[strt]+1
        else:
            s.update({strt:1})
    return json.dumps(s)

if __name__ == '__main__':
    app.jinja_env.auto_reload = True
    app.config['TEMPLATES_AUTO_RELOAD'] = True
    app.run(host='0.0.0.0', debug=True, port=80)