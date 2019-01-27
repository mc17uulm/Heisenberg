const fs = require('fs');

class Handler {

    static getConfigFile()
    {
        return "./../../Assets/StreamingAssets/config.json";
    }

    static async getAll() {
        let data = await JSON.parse(fs.readFileSync(this.getConfigFile()));
        return data;
    }

    static async get(id) {
        let data = await JSON.parse(fs.readFileSync(this.getConfigFile()));
        return data.filter((el) => {
            el.id === id
        });
    }

    static async add(obj) {
        let data = await JSON.parse(fs.readFileSync(this.getConfigFile()));
        let lastId = Math.max.apply(Math, data.data.map((el) => {
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
        data.data.push(user);
        await fs.writeFileSync(this.getConfigFile(), JSON.stringify(data));
        return user;
    }

}

module.exports = Handler;