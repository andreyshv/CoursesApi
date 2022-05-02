import React, { Component } from "react";
import { Routes, Route, Link } from "react-router-dom";
import { useParams } from "react-router-dom";
//import {  } from "react-router";
//import { Layout } from "./components/Layout";
import { Students } from "./components/Students";

import "./custom.css";

export default class App extends Component {
  static displayName = App.name;

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
  if (params.hasOwnProperty("studentId")) {
    console.log(params.studentId);
    let student = getStudent(parseInt(params.studentId, 10));
  }

  return (
    <div>
      <h1>Student {params.studentId}</h1>
      <nav>
        <Link to="/">List</Link>
      </nav>
    </div>
  );
}
