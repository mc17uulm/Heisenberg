import React, {Component} from 'react';

class Input extends Component
{

    constructor(props)
    {
        super(props);
        this.state = {
            value: this.props.value
        }

        this.handleChange = this.handleChange.bind(this);
        this.reset = this.reset.bind(this);
    }

    handleChange(e) {
        this.props.onChange({
            label: this.props.labelId,
            value: e.target.value
        });
        this.setState({
            value: e.target.value
        });
    }

    reset() {
        this.setState({
            value: ""
        });
    }

    render() {
        return (
            <div className="form-group">
                <label htmlFor={this.props.labelId}>{this.props.label}</label>
                <input type={this.props.type} className="form-control" id={this.props.labelId} value={this.state.value} onChange={this.handleChange} />
            </div>
        )
    }
}

export default Input;