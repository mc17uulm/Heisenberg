import React, {Component} from 'react';
import Form from './Form.jsx';
import Table from './Table.jsx';
import Getter from './Getter.js';
import Modal from "./Modal.jsx";

class App extends Component
{

    constructor(props) {
        super(props);

        this.state = {
            data: [],
            refreshText: "Refresh",
            modalHidden: true
        }

        this.add = this.add.bind(this);
        this.handleRefresh = this.handleRefresh.bind(this);
        this.handleSettings = this.handleSettings.bind(this);
        this.handleSaveSettings = this.handleSaveSettings.bind(this);
        this.toggelModal = this.toggelModal.bind(this);
    }

    async componentDidMount() {
        let d = await Getter.sendRequest(JSON.stringify({
            type: "get_all"
        }));
        this.setState({ 
            data: d.data
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
        let d = await Getter.sendRequest(JSON.stringify({
            type: "get_all"
        }));
        this.setState({ 
            data: d.data,
            refreshText: "Refresh"
        });
    }

    handleSettings(e) {
        e.preventDefault();
        this.setState({
            modalHidden: false
        });
    }

    toggelModal() {
        this.setState({
            modalHidden: !this.state.modalHidden
        });
    }

    async handleSaveSettings(e)
    {
        e.preventDefault();
    }

    render() {
        return (
            <div>
                <hr />
                <div className="row">
                    <div className="col-11">
                        <h3>Heisenberg Project</h3>
                    </div>
                    <div className="col-1">
                        <button type="button" onClick={this.handleSettings} className="btn btn-dark float-right">Settings</button>
                    </div>
                </div>
                <Form add={this.add} />
                <br/>
                <Table data={this.state.data}/>
                <button type="button" onClick={this.handleRefresh} className="btn btn-info">{this.state.refreshText}</button>
                <Modal title="Settings" body="" onSave={this.handleSaveSettings} toggleHidden={this.toggelModal} hidden={this.state.modalHidden}/>
                {this.state.modalHidden ? "" : (
                    <div className="modal-backdrop fade show"></div>
                )}
            </div>
        );
    }

}

export default App;