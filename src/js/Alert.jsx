import React, {Component} from 'react';

class Alert extends Component
{

    constructor(props)
    {
        super(props);
    }

    render() {
        return (
            <div className={"alert alert-" + this.props.type + " alert-dismissible fade show"} role="alert">
                <button type="button" className="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
                <strong>{this.props.title}</strong> {this.props.message}
            </div>
        )
    }

}

export default Alert;