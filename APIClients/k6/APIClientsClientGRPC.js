import grpc from 'k6/net/grpc';
import { check, sleep } from "k6";

let URL = 'localhost:6001';

let client = new grpc.Client();
client.load(['../Protos'], 'APIClients.proto');


export default () => {
    client.connect(URL, {})
    
    let data = {
        Name: 'Guy',
        Street: 'Street 1',
        Local: 'Localized',
        PostalCode: '4444-444',
        Country: 'US'
    };

    let response = client.invoke('APIClientsRPC.APIClientsClientGRPC/AddNewClient', data);
    check(response, {
        'status is ok Add': (r) => r.status === grpc.StatusOK,
    });

    let clientId = response.message.ClientId;

    //console.log(clientId)

    let dataTagOnly =
    {
        Id: response.message.ClientId
    }


    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/GetClientByID', dataTagOnly);
    check(response, {
        'status is ok Get': (r) => r.status === grpc.StatusOK,
    });

    data = {
        ClientId: clientId,
        Name: 'Guy Fiery',
        Street: 'Street 1',
        Local: 'Localized',
        PostalCode: '4444-444',
        Country: 'US'
    };

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/ModifyClient', data);
    check(response, {
        'status is ok after changing Name': (r) => r.status === grpc.StatusOK,
        'Name change success': (r) => r.message.Name === 'Guy Fiery'
    });

    data = {
        ClientId: clientId,
        Name: 'Guy Fiery',
        Street: 'Street 2',
        Local: 'Localized',
        PostalCode: '4444-444',
        Country: 'US'
    };

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/ModifyClient', data);
    check(response, {
        'status is ok after changing Street': (r) => r.status === grpc.StatusOK,
        'Street change success': (r) => r.message.Street === 'Street 2'
    });

    data = {
        ClientId: clientId,
        Name: 'Guy Fiery',
        Street: 'Street 2',
        Local: 'Unlocalized',
        PostalCode: '4444-444',
        Country: 'US'
    };

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/ModifyClient', data);
    check(response, {
        'status is ok after changing Local': (r) => r.status === grpc.StatusOK,
        'Local change success': (r) => r.message.Local === 'Unlocalized'
    });

    data = {
        ClientId: clientId,
        Name: 'Guy Fiery',
        Street: 'Street 2',
        Local: 'Unlocalized',
        PostalCode: '4444-666',
        Country: 'US'
    };

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/ModifyClient', data);
    check(response, {
        'status is ok after changing PostalCode': (r) => r.status === grpc.StatusOK,
        'PostalCode change success': (r) => r.message.PostalCode === '4444-666'
    });

    data = {
        ClientId: clientId,
        Name: 'Guy Fiery',
        Street: 'Street 2',
        Local: 'Unlocalized',
        PostalCode: '4444-666',
        Country: 'ZA'
    };

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/ModifyClient', data);
    check(response, {
        'status is ok after changing Country': (r) => r.status === grpc.StatusOK,
        'Country change success': (r) => r.message.Country === 'ZA'
    });

    response = client.invoke('APIClientsRPC.APIClientsClientGRPC/DeleteClient', dataTagOnly);
    check(response, {
        'status is ok delete': (r) => r.status === grpc.StatusOK,
    });

    client.close()
}