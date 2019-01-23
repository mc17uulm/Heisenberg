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
                <th>{this.props.vorname}</th>
                <th>{this.props.nachname}</th>
                <th>{new Date(this.props.date).toLocaleDateString("de-DE") + " " + new Date(this.props.date).toLocaleTimeString("de-DE")}</th>
                <th><span className={"badge badge-" + (this.props.state === "added" ? "warning" : "success")}>{this.props.state}</span></th>
                <th>{this.props.files.map((el) => (
                    <div>
                        <a href={el.link}>{el.name}</a><br/>
                    </div>
                ))}</th>
            </tr>
        );
    }

}

export default TableRow;