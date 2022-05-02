import React, { Component } from "react";
import { Routes, Route, Link } from "react-router-dom";
import { useParams } from "react-router-dom";
//import {  } from "react-router";
//import { Layout } from "./components/Layout";
import { Students } from "./components/Students";

import "./custom.css";

export default class App extends Component {
  render() {
    return (
      <div>
        <h1>Application</h1>
        <Routes>
          <Route path="/" element={<Students />} />
          <Route path="/student" element={<Student />}>
            <Route path="/student/:studentId" element={<Student />} />
          </Route>
        </Routes>
      </div>
    );
    // <Layout>
    //   <Route exact path='/' component={Home} />
    //   <Route path='/counter' component={Counter} />
    //   <Route path='/fetch-data' component={FetchData} />
    // </Layout>
  }
}

function getStudent(id) {
  return {};
}

function Student() {
  let params = useParams();
  let student = {};
  if (params.hasOwnProperty("studentId")) {
    console.log(params.studentId);
    student = getStudent(parseInt(params.studentId, 10));
  }

  return (
    <div>
      <h1>Student {params.studentId}</h1>
      <p>Name {student.Name}</p>
      <nav>
        <Link to="/">List</Link>
      </nav>
    </div>
  );
}
