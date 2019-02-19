const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const handler = require('./Handler.js');
const app = express();

app.use(cors());
app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

app.get('/', (req, res) => {
    res.send("API expects POST requests only");
});

app.post('/', async (req, res) => {
    res.type('json');
    if(typeof req.body.type !== "undefined") {
        switch(req.body.type) {
            case "get_all":
                let data = await handler.getAll();
                res.send(JSON.stringify({type: "success", msg: data}));
                break;
            case "get":
                if(typeof req.body.data.id !== "undefined") {
                    res.send(await JSON.stringify({type: "success", msg: await handler.get(req.body.data.id)}));
                }  else {
                    res.send(JSON.stringify({type: "error", msg: "Invalid requests1"}));
                } 
                break;
            case "add":
                if(typeof req.body.data.vorname !== "undefined" &&  typeof req.body.data.nachname !== "undefined") {
                    res.send(await JSON.stringify({type: "success", msg: await handler.add(req.body.data)}));
                } else {
                    res.send(JSON.stringify({type: "error", msg: "Invalid requests2"}));
                }
                break;
            default:
                res.send(JSON.stringify({type: "error", msg: "Invalid requests3"}));
                break;        
        }
    } else {
        res.send(JSON.stringify({type: "error", msg: "Invalid requests4"}));
    }
});

app.listen(8081, () => {
    console.log("API server is listening on port 8081");
});