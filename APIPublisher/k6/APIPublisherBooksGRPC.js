import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:7001';

let client = new grpc.Client();
client.load(['../Protos'], 'APIPublisher.proto');


export default () => {
    client.connect(URL, {})

    let dataTagOnly =
    {
        Id: '978-0000000000'
    }

    let response = client.invoke('APIPublisherRPC.APIPublisherBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is not found': (r) => r && r.status === grpc.StatusNotFound,
    });

    let data = {
        PublisherId: 'PU1',
        Name: 'Pub1',
        Country: 'US'
    };

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/AddNewPublisher', data);
    check(response, {
        'status is ok Add 1': (r) => r.status === grpc.StatusOK,
    });

    data = {
        PublisherId: 'PU2',
        Name: 'Pub2',
        Country: 'ZA'
    };

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/AddNewPublisher', data);
    check(response, {
        'status is ok Add 2': (r) => r.status === grpc.StatusOK,
    });

    data = {
        Id: "978-0000000000",
        PublisherId: 'PU1'   
    };

    response = client.invoke('APIPublisherRPC.APIPublisherBooksGRPC/AddNewBook', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    response = client.invoke('APIPublisherRPC.APIPublisherBooksGRPC/GetBookByISBN', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
        'Right isbn': (r) => r.message.Id === '978-0000000000',
        'Right Pub': (r) => r.message.Publisher.PublisherId === 'PU1'
    });

    data = {
        Id: "978-0000000000",
        PublisherId: 'PU2'    
    };

    response = client.invoke('APIPublisherRPC.APIPublisherBooksGRPC/ModifyBook', data);
    check(response, {
        'status is ok after changing Publisher': (r) => r.status === grpc.StatusOK,
        'Publisher change success': (r) => r.message.Publisher.PublisherId === 'PU2'
    });

    response = client.invoke('APIPublisherRPC.APIPublisherBooksGRPC/DeleteBook', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'PU1'
    }

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/DeletePublisher', dataTagOnly);
    check(response, {
        'status is ok delete1': (r) => r.status === grpc.StatusOK,
    });

    dataTagOnly =
    {
        Id: 'PU2'
    }

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/DeletePublisher', dataTagOnly);
    check(response, {
        'status is ok delete2': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}