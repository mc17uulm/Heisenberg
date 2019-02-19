class Getter
{

    static async sendRequest(body)
    {
        const resp = await fetch(
            "http://localhost:8081/",
            {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                method: 'POST',
                body: body
            }
        );
        
        const content = await resp.json();

        return content.msg;
    }

    static async getAll()
    {
        return await this.sendRequest('all');
    }

    static async getFromId(id)
    {
        return await this.sendRequest(id);
    }
}

export default Getter;