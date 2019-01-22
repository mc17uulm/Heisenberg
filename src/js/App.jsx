import React, {Component} from 'react';
import Form from './Form.jsx';
import Table from './Table.jsx';
import Getter from './Getter.js';

class App extends Component
{

    constructor(props) {
        super(props);

        this.state = {
            data: [],
            refreshText: "Refresh"
        }

        this.add = this.add.bind(this);
        this.handleRefresh = this.handleRefresh.bind(this);
    }

    async componentDidMount() {
        this.setState({ 
            data: await Getter.sendRequest(JSON.stringify({
                type: "get_all"
            }))
        });
    }

    async add(obj){
        let data = this.state.data;
        data.push(await Getter.sendRequest(
            JSON.stringify({
                type: "add",
                data: obj
            })
        ));

        this.setState({
            data: data
        });
    }

    async handleRefresh(e) {
        e.preventDefault();
        this.setState({
            refreshText: "Loading ..."
        });
        this.setState({ 
            data: await Getter.sendRequest(JSON.stringify({
                type: "get_all"
            })),
            refreshText: "Refresh"
        });
    }

    render() {
        return (
            <div>
                <h3>Heisenberg Project</h3>
                <Form add={this.add} />
                <br/>
                <Table data={this.state.data}/>
                <button type="button" onClick={this.handleRefresh} className="btn btn-info">{this.state.refreshText}</button>
            </div>
        );
    }

}

export default App;