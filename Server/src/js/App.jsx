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
        this.delete = this.delete.bind(this);
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

    async add(){
        let data = this.state.data;
        data.push(await Getter.sendRequest(
            JSON.stringify({
                type: "add"
            })
        ));

        this.setState({
            data: data
        });
    }

    async delete(id)
    {
        const resp = await Getter.sendRequest(
            JSON.stringify({
                type: "delete",
                id: id
            })
        );
        if(resp)
        {
            let data = this.state.data;
            let newData = data.filter((el) => {
                return el.id !== id;
            });
            this.setState({
                data: newData
            });
        }
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
                <Table data={this.state.data} delete={this.delete}/>
                <div className="btn-toolbar mb-3" role="toolbar" aria-label="Toolbar">
                    <div className="mr-2">
                        <button type="button" onClick={this.handleRefresh} className="btn btn-info">{this.state.refreshText}</button>
                    </div>
                    <div>
                        <button type="button" onClick={this.add} className="btn btn-success">Add</button>
                    </div>
                </div>
                <Modal title="Settings" body="" onSave={this.handleSaveSettings} toggleHidden={this.toggelModal} hidden={this.state.modalHidden}/>
                {this.state.modalHidden ? "" : (
                    <div className="modal-backdrop fade show"></div>
                )}
            </div>
        );
    }

}

export default App;