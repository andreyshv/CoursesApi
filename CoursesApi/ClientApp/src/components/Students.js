import React, { Component } from "react";
import Button from "react-bootstrap/Button";
import { Link, useNavigate } from "react-router-dom";

export class Students extends Component {
  static displayName = Students.name;

  constructor(props) {
    super(props);
    this.state = { students: [], loading: true };
  }

  componentDidMount() {
    this.populateData();
  }

  renderStudentsTable(students) {
    return (
      <div>
        <Link to={"/student"}>Add</Link>
        <table className="table table-striped" aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>Full Name</th>
              <th>E-Mail</th>
            </tr>
          </thead>
          <tbody>
            {students.map((student) => (
              <tr key={student.id}>
                <td>{student.fullName}</td>
                <td>{student.email}</td>
                <td>
                  <Link to={`/student/${student.id}`}>Edit | </Link>
                  <Link to={`/courses/${student.id}`}>Courses | </Link>
                  <Button
                    variant="danger"
                    onClick={() => this.handleRemove(student.id)}
                  >
                    Delete
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  render() {
    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
      this.renderStudentsTable(this.state.students)
    );

    return (
      <div>
        <h1 id="tabelLabel">Students</h1>
        {contents}
      </div>
    );
  }

  async populateData() {
    const response = await fetch("/api/students");
    const data = await response.json();
    this.setState({ students: data, loading: false });
  }

  async handleRemove(id) {
    const response = await fetch(`/api/students/${id}`, { method: "DELETE" });

    const students = this.state.students.filter((student) => student.id !== id);
    this.setState({ students: students });
  }
}
