import React, {Component} from 'react';
import Input from './Input.jsx';
import Alert from './Alert.jsx';

class Form extends Component
{

    constructor(props)
    {
        super(props);

        this.state = {
            vorname: "",
            nachname: "",
            alert: false
        }

        this.child_v = React.createRef();
        this.child_n = React.createRef();
        this.add = this.add.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }

    add(e) {
        e.preventDefault();
        this.setState({
            alert: this.state.vorname === "" || this.state.nachname === "" || this.state.vorname === "undefined" || this.state.nachname === "undefined"
        });
        let vorname = this.state.vorname,
            nachname = this.state.nachname;
        if(!this.state.alert){
            this.setState({
                vorname: "",
                nachname: ""
            });
            this.child_v.current.reset();
            this.child_n.current.reset();
        }
        this.props.add({
            vorname: vorname,
            nachname: nachname
        });
    }

    handleChange(el)
    {
        this.setState({
            [el.label]: el.value,
            alert: false
        });
    }

    render() {
        return (
            <div>
                <form onSubmit={this.add}>
                    <Input ref={this.child_v} label="Vorname" labelId="vorname" type="text" value={this.state.vorname} onChange={this.handleChange} />
                    <Input ref={this.child_n} label="Nachname" labelId="nachname" type="text" value={this.state.nachname} onChange={this.handleChange} />
                    {this.state.alert ? (
                        <Alert type="danger" title="Achtung!" message="Bitte gib einen Vornamen und Namen ein!" />
                    ) : ""}
                    <button type="submit" className="btn btn-success">Add</button>
                </form>
            </div>
        )
    }

}

export default Form;