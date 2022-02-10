const express = require("express");
const app = express();

app.get('/', (req, res) => {
    res.send(req.headers);
});


const SERVER_PORT = process.env.PORT || 3000;
app.listen(SERVER_PORT, () => console.log(`服务器已经启动在: http://localhost:${SERVER_PORT}`));
