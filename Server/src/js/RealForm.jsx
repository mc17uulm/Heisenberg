import React, {Component} from "react";

class RealForm extends Component
{

    constructor(props)
    {
        super(props);

        this.state = {
            forms: [],
            alert: false,
            alertMsg: ""
        }

        this.submit = this.submit.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }

    componentDidMount()
    {
        let forms = [];
        this.props.forms.forEach((el) => {
            forms.push({
                id: el.name.toLowerCase(),
                label: el.name,
                type: el.type,
                value: "",
                ref: React.createRef()
            });
        });
        this.setState({
            forms: forms
        });
    }

    submit(e) {
        e.preventDefault();
        let out = [];
        this.state.forms.every((el) => {
            if(el.value === "" || el.value === "undefined") {
                this.setState({
                    alert: true,
                    alertMsg: "Please insert all required fields!"
                });
                return false;
            }
            out.push({
                id: el.id,
                value: el.value
            });
            el.ref.current.reset();
            return true;
        });

        this.setState({
            forms: this.state.forms.map((el) => {
                return {
                    id: el.id,
                    label: el.label,
                    type: el.type,
                    value: "",
                    ref: el.ref
                }
            })
        });
        this.props.recieve(out);
    }

    handleChange(e){
        e.preventDefault();
        this.setState({
            forms: this.state.forms.map((el) => {
                if(el.label === e.labelId){
                    return {
                        id: el.id,
                        label: el.label,
                        type: el.type,
                        value: e.value,
                        ref: el.ref
                    };
                } else {
                    return el;
                }
            }),
            alert: false
        });
    }

    render() {
        return (
            <form onSumbit={this.submit}>
                {this.state.forms.forEach((el) => (
                    <Input ref={el.ref} type={el.type} label={el.label} labelId={el.id} value={el.value} onChange={this.handleChange} />
                ))}
                {this.state.alert ? (
                    <Alert type="danger" title="Important!" message={this.state.alertMsg}/>
                ) : ""}
                <button type="submit" className="btn btn-success">{this.props.btn}</button>
            </form>
        );
    }

}

export default RealForm;