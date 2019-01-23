const fs = require('fs');

class Handler {

    static async getAll() {
        let data = await JSON.parse(fs.readFileSync('./../config.json'));
        return data;
    }

    static async get(id) {
        let data = await JSON.parse(fs.readFileSync('./../config.json'));
        return data.filter((el) => {
            el.id === id
        });
    }

    static async add(obj) {
        let data = await JSON.parse(fs.readFileSync('./../config.json'));
        let lastId = Math.max.apply(Math, data.map((el) => {
            return el.id;
        }));
        if(!Number.isInteger(lastId)) {
            lastId = 0;
        }
        let now = Date.now();
        let user = {
            id: lastId += 1,
            vorname: obj.vorname,
            nachname: obj.nachname,
            state: "added",
            date: now,
            files: []
        };
        data.push(user);
        await fs.writeFileSync('./../config.json', JSON.stringify(data));
        // let csFile = await fs.readFileSync("./../Config.cs").toString();
        // let parts = csFile.split("=");
        // let second = parts[1].split(";");
        // let third = parts[2].split(";");
        // let newCs =  parts[0] + "= \"" + obj.vorname + " " + obj.nachname + "\";" + second[1] + "= " + lastId + ";" + third[1];
        // for(let i = 0; i < 3; i++){
        //     parts.shift();
        // }
        // newCs += parts.join("=");
        // await fs.writeFileSync("./../Config.cs", newCs);
        return user;
    }

}

module.exports = Handler;