import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:7001';

let client = new grpc.Client();
client.load(['../Protos'], 'APIPublisher.proto');


export default () => {
    client.connect(URL, {})
    
    let dataTagOnly =
    {
        Id: 'PU1'
    }

    let response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/GetPublisherByID', dataTagOnly);
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
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/GetPublisherByID', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
    });

    data = {
        PublisherId: 'PU1',
        Name: 'Pub2',
        Country: 'US'
    };

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/ModifyPublisher', data);
    check(response, {
        'status is ok after changing Name': (r) => r.status === grpc.StatusOK,
        'Name change success': (r) => r.message.Name === 'Pub2'
    });

    data = {
        PublisherId: 'PU1',
        Name: 'Pub2',
        Country: 'ZA'
    };

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/ModifyPublisher', data);
    check(response, {
        'status is ok after changing Country': (r) => r.status === grpc.StatusOK,
        'Country change success': (r) => r.message.Country === 'ZA'
    });

    response = client.invoke('APIPublisherRPC.APIPublisherPublisherGRPC/DeletePublisher', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}