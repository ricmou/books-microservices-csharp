import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:5001';

let client = new grpc.Client();
client.load(['../Protos'], 'APIAuthors.proto');


export default () => {
    client.connect(URL, {})
    
    let dataTagOnly =
    {
        Id: 'TE1'
    }

    let response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/GetAuthorByID', dataTagOnly);
    check(response, {
        'Call 1: Expect NOT_FOUND (5) from GetAuthorByID': (r) => r && r.status === grpc.StatusNotFound,
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
        'Call 2: Expect OK (0) from AddNewAuthor': (r) => r.status === grpc.StatusOK,
    });


    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/GetAuthorByID', dataTagOnly);
    check(response, {
        'Call 3: Expect OK (0) from GetAuthorByID': (r) => r.status === grpc.StatusOK,
    });

    data = {
        AuthorId: 'TE1',
        BirthDate: '01/09/2038',
        Country: 'US',
        FirstName: 'FirstName1',
        LastName: 'LastName'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/ModifyAuthor', data);
    check(response, {
        'Call 4: Expect OK (0) From ModifyAuthor': (r) => r.status === grpc.StatusOK,
        'Call 4: Check for successful FirstName change': (r) => r.message.FirstName === 'FirstName1'
    });

    data = {
        AuthorId: 'TE1',
        BirthDate: '01/09/2038',
        Country: 'US',
        FirstName: 'FirstName1',
        LastName: 'LastName1'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/ModifyAuthor', data);
    check(response, {
        'Call 5: Expect OK (0) From ModifyAuthor': (r) => r.status === grpc.StatusOK,
        'Call 5: Check for successful LastName change': (r) => r.message.LastName === 'LastName1'
    });

    data = {
        AuthorId: 'TE1',
        BirthDate: '11/09/2038',
        Country: 'US',
        FirstName: 'FirstName1',
        LastName: 'LastName1'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/ModifyAuthor', data);
    check(response, {
        'Call 6: Expect OK (0) From ModifyAuthor': (r) => r.status === grpc.StatusOK,
        'Call 6: Check for successful BirthDate change': (r) => r.message.BirthDate === '11/09/2038'
    });

    data = {
        AuthorId: 'TE1',
        BirthDate: '11/09/2038',
        Country: 'ZA',
        FirstName: 'FirstName1',
        LastName: 'LastName1'
    };

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/ModifyAuthor', data);
    check(response, {
        'Call 7: Expect OK (0) From ModifyAuthor': (r) => r.status === grpc.StatusOK,
        'Call 7: Check for successful Country change': (r) => r.message.Country === 'ZA'
    });

    response = client.invoke('APIAuthorsRPC.APIAuthorsAuthorGRPC/DeleteAuthor', dataTagOnly);
    check(response, {
        'Call 8: Expect OK (0) From DeleteAuthor': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}