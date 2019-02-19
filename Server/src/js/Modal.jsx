import React, {Component} from "react";
import RealFrom from "./RealForm.jsx";

class Modal extends Component
{

    constructor(props)
    {
        super(props);
        this.recieve = this.recieve.bind(this);
    }

    recieve(res) {

    }

    render() {
        return (
                <div className={`modal fade${this.props.hidden ? "" : " show"}`} tabindex="-1" style={{display: this.props.hidden ? "none" : "block"}} role="dialog">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">{this.props.title}</h5>
                                <button type="button" onClick={this.props.toggleHidden} aria-label="close" className="close" ><span aria-hidden="true">&times;</span></button>
                            </div>
                            <div className="modal-body">
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" onClick={this.props.toggleHidden}>Close</button>
                                <button type="button" className="btn btn-success" onClick={this.props.onSave}>Save</button>
                            </div>
                        </div>
                    </div>
                </div>
        );
    }

}

export default Modal;