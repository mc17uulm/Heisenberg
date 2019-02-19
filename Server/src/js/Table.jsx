import React, {Component} from 'react';
import TableRow from './TableRow.jsx';

class Table extends Component
{

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <table className="table">
                <thead className="thead-dark">
                    <tr>
                        <th scope="col">ID</th>
                        <th scope="col">Vorname</th>
                        <th scope="col">Nachname</th>
                        <th scope="col">Date</th>
                        <th scope="col">State</th>
                        <th scope="col">Files</th>
                        <th scope="col">Delete</th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.data.slice(0).reverse().map((el, i) => (
                        <TableRow type={i === 0 ? "table-success" : ""} id={el.id} vorname={el.vorname} nachname={el.nachname} date={el.date} files={el.files} state={el.state} delete={() => {this.props.delete(el.id)}}/>
                    ))}
                </tbody>
            </table>
        );
    }

}

export default Table;