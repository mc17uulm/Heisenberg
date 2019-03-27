import React, {Component} from "react";

class TableRow extends Component
{

    constructor(props)
    {
        super(props);
    }

    render() {
        return (
            <tr className={this.props.type}>
                <th scope="row">{this.props.id}</th>
                <th>{new Date(parseInt(this.props.date)).toLocaleDateString("de-DE") + " " + new Date(this.props.date).toLocaleTimeString("de-DE")}</th>
                <th><span className={"badge badge-" + (this.props.state === "added" ? "info" : "success")}>{this.props.state}</span></th>
                <th>{this.props.files.map((el) => (
                    <div>
                        <a href={el}>{el.substr(el.lastIndexOf("/")+1, el.length)}</a><br/>
                    </div>
                ))}</th>
                <th>
                    {this.props.state === "added" ? (
                        <button type="button" className="btn btn-danger" onClick={this.props.delete}>Delete</button>
                    ) : ""}
                </th>
            </tr>
        );
    }

}

export default TableRow;