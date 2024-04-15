import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:6501';

let client = new grpc.Client();
client.load(['../Protos'], 'APIExemplar.proto');


export default () => {
    client.connect(URL, {})
    
    let data = {
        BookId: '978-0000000000',
        BookState: 2,
        SellerId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        DateOfAcquisition: '05/01/2017'
    };

    let response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/AddNewExemplar', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    let exemplarId = response.message.ExemplarId;

    //console.log(ExemplarId)

    let dataTagOnly =
    {
        Id: response.message.ExemplarId
    }


    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/GetExemplarByID', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
    });

    data = {
        ExemplarId: exemplarId,
        BookId: '978-0000000001',
        BookState: 2,
        SellerId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        DateOfAcquisition: '05/01/2017'
    };

    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/ModifyExemplar', data);
    check(response, {
        'status is ok after changing BookId': (r) => r.status === grpc.StatusOK,
        'BookId change success': (r) => r.message.BookId === '978-0000000001'
    });

    data = {
        ExemplarId: exemplarId,
        BookId: '978-0000000001',
        BookState: 3,
        SellerId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        DateOfAcquisition: '05/01/2017'
    };

    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/ModifyExemplar', data);
    check(response, {
        'status is ok after changing BookState': (r) => r.status === grpc.StatusOK,
        'BookState change success': (r) => r.message.BookState === 3
    });

    data = {
        ExemplarId: exemplarId,
        BookId: '978-0000000001',
        BookState: 3,
        SellerId: '11111111-1111-1111-1111-111111111111',
        DateOfAcquisition: '05/01/2017'
    };

    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/ModifyExemplar', data);
    check(response, {
        'status is ok after changing SellerId': (r) => r.status === grpc.StatusOK,
        'SellerId change success': (r) => r.message.SellerId === '11111111-1111-1111-1111-111111111111'
    });

    data = {
        ExemplarId: exemplarId,
        BookId: '978-0000000001',
        BookState: 3,
        SellerId: '11111111-1111-1111-1111-111111111111',
        DateOfAcquisition: '15/01/2017'
    };

    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/ModifyExemplar', data);
    check(response, {
        'status is ok after changing DateOfAcquisition': (r) => r.status === grpc.StatusOK,
        'DateOfAcquisition change success': (r) => r.message.DateOfAcquisition === '15/01/2017'
    });

    response = client.invoke('APIExemplarRPC.APIExemplarExemplarGRPC/DeleteExemplar', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}