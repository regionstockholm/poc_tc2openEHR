export interface IBetterConfig {
        UserName: string,
        Password: string
        EndpointUrl : string,
        EndpointAdminUrl : string
}


export interface IConfigHolder{
    ConfigData : IBetterConfig
} 