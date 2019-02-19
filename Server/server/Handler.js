const fs = require('fs');

class Handler {

    static getConfigFile()
    {
        //return "./../config.json";
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

    static async add() {
        let data = await JSON.parse(fs.readFileSync(this.getConfigFile()));
        let lastId = Math.max.apply(Math, data.data.map((el) => {
            return el.id;
        }));
        if(!Number.isInteger(lastId)) {
            lastId = 0;
        }
        let user = {
            id: lastId += 1,
            state: "added",
            date: Date.now(),
            files: []
        };
        data.data.push(user);
        await fs.writeFileSync(this.getConfigFile(), JSON.stringify(data));
        return user;
    }

    static async delete(id)
    {
        let data = await JSON.parse(fs.readFileSync(this.getConfigFile()));
        let newData = data.data.filter((el) => {
            return el.id !== id;
        });
        let resp = data.data.length !== newData.length;
        data.data = newData;
        await fs.writeFileSync(this.getConfigFile(), JSON.stringify(data));
        return resp;
    }

}

module.exports = Handler;