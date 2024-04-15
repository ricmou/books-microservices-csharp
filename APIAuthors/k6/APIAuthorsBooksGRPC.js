import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:5001';

let client = new grpc.Client();
client.load(['../Protos'], 'APIAuthors.proto');


export default () => {
    client.connect(URL, {})

    let dataTagOnly =
    {
        Id: '978-0000000000'
    }

    let response = client.invoke('APIAuthorsRPC.APIAuthorsBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is not found': (r) => r && r.status === grpc.StatusNotFound,
    });

    let data = {
        AuthorId: 'TE1',
        BirthDate: '01/09/2038',
        Country: 'US',
        FirstName: 'FirstName',
        LastName: 'LastName'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/AddNewAuthor', data);
    check(response, {
        'status is ok Add 1': (r) => r.status === grpc.StatusOK,
    });

    data = {
        AuthorId: 'TE2',
        BirthDate: '01/09/2038',
        Country: 'US',
        FirstName: 'FirstName',
        LastName: 'LastName'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/AddNewAuthor', data);
    check(response, {
        'status is ok Add 2': (r) => r.status === grpc.StatusOK,
    });

    data = {
        Id: "978-0000000000",
        Authors: ["TE1"]    
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsBooksGRPC/AddNewBook', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    response = client.invoke('APIAuthorsRPC.APIAuthorsBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
        'Right isbn': (r) => r.message.Id === '978-0000000000',
        'Right Author': (r) => r.message.Authors[0].AuthorId === 'TE1'
    });

    data = {
        Id: "978-0000000000",
        Authors: ["TE2"]    
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsBooksGRPC/ModifyBook', data);
    check(response, {
        'status is ok after changing Authors': (r) => r.status === grpc.StatusOK,
        'Authors change success': (r) => r.message.Authors[0].AuthorId === 'TE2'
    });

    response = client.invoke('APIAuthorsRPC.APIAuthorsBooksGRPC/DeleteBook', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'TE1'
    }

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/DeleteAuthor', dataTagOnly);
    check(response, {
        'status is ok delete1': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'TE2'
    }

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/DeleteAuthor', dataTagOnly);
    check(response, {
        'status is ok delete2': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}