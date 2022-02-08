const express = require("express");
const msal = require("@azure/msal-node");

const SERVER_PORT = process.env.PORT || 3000;

const app = express();

const config = {
    auth: {
        clientId: "b342d605-dbf8-4fd8-ac21-daddc664984c",
        authority: "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd",
        clientSecret: "DVf7Q~SGK9Iywggz9mrdVzfYy5wHyXZrb4dTe"
    }
}

const cca = new msal.ConfidentialClientApplication(config);

app.get("/", (req, res) => {
    const authCodeUrlParameters = {
        scopes: ['user.read'],
        redirectUri: "http://localhost:3000/redirect"
    };

    cca.getAuthCodeUrl(authCodeUrlParameters).then(response => res.redirect(response)).catch((error) => console.log(JSON.stringify(error)));

});


app.get('/redirect', (req, res) => {
    const tokenRequest = {
        code: req.query.code,
        scopes: ['user.read'],
        redirectUri: 'http://localhost:3000/redirect'
    };

    cca.acquireTokenByCode(tokenRequest).then((response) => {
        console.log(response);
        res.sendStatus(200);
    }).catch((error) => {
        console.log(error);
        res.status(500).send(error);
    })
})

app.listen(SERVER_PORT, () => console.log("服务器已经启动"));
