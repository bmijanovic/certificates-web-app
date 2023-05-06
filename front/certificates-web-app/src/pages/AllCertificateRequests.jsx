import React, {useContext, useEffect, useState} from "react";
import {useQuery} from "@tanstack/react-query";
import CertificateRequestCard from "../components/CertificateRequestCard.jsx";
import axios from "axios";
import {
    Box,
    FormControl,
    Grid,
    InputLabel,
    MenuItem,
    Select,
    Tab,
    TablePagination,
    Tabs,
    Typography
} from "@mui/material";
import { TabPanel, TabContext } from '@mui/lab';
import {AuthContext} from "../security/AuthContext.jsx";
import CertificateCard from "../components/CertificateCard.jsx";



export default function AllCertificateRequests() {
    const [value, setValue] = useState(0);
    const [page, setPage] = React.useState(0);
    const [totalCount, setTotalCount] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(1);
    const [certificateRequests,setCertificateRequests] = React.useState([]);

    const { isAuthenticated, role, isLoading } = useContext(AuthContext);


    const certificateRequestsQuery = useQuery({
        queryKey: ["certificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest").then(res => res.data).catch(err => {console.log(err)})
    })

    const allCertificateRequestsQuery = useQuery({
        queryKey: ["allCertificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest/getAll").then(res => res.data).catch(err => {console.log(err)})
    })

    const approvalCertificateRequestsQuery = useQuery({
        queryKey: ["approvalCertificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest/forApproval").then(res => res.data).catch(err => {console.log(err)})
    })

    const handleChange = (event, newValue) => {
        setValue(newValue.props.value);
        setPage(0);

    };

    const handleChangePage = (
        event, newPage) => {
        setPage(newPage);
    };
    const handleChangeRowsPerPage = (
        event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    useEffect(() => {
        switch (value) {
            case 0:
                axios.get(`https://localhost:7018/api/CertificateRequest?PageNumber=${page + 1}&PageSize=${rowsPerPage}`).then(res => {
                    setTotalCount(res.data.totalCount);
                    setCertificateRequests(res.data.certificatesRequest)
                }).catch(err => {
                    console.log(err)
                });
                break;
            case 1:
                axios.get(`https://localhost:7018/api/CertificateRequest/forApproval?PageNumber=${page + 1}&PageSize=${rowsPerPage}`).then(res => {
                    setTotalCount(res.data.totalCount);
                    setCertificateRequests(res.data.certificatesRequest)
                }).catch(err => {
                    console.log(err)
                });
                break;
            case 2:
                axios.get(`https://localhost:7018/api/CertificateRequest/getAll?PageNumber=${page + 1}&PageSize=${rowsPerPage}`).then(res => {
                    setTotalCount(res.data.totalCount);
                    setCertificateRequests(res.data.certificatesRequest)
                }).catch(err => {
                    console.log(err)
                });
                break;
        }
    },[page,rowsPerPage,value])

    const renderPanel = () => {
        return <>
            {certificateRequests.length===0 ? <p>Loading...</p> : <div>
                <Grid container sx={{bx:3, mt:1}} spacing={25}>
                    {certificateRequests.map(item => <CertificateRequestCard key={item.id} data={item} acceptable={false}/>)}
                </Grid>
            </div>
            }
        </>

    };

    return <>
        <div style={{textAlign: "center", width: "80%", margin:"auto"}}>
            <h1>Certificate Requests</h1>
            <FormControl >
                <InputLabel id="demo-simple-select-label">Certificate requests</InputLabel>
                <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    value={value}
                    label="Certificate requests"
                    onChange={handleChange}>
                    <MenuItem value={0}>Requests for your certificates</MenuItem>
                    <MenuItem value={1}>Request based on your certificates</MenuItem>
                    <MenuItem value={2}>All requests</MenuItem>
                </Select>
            </FormControl>
            {renderPanel()}
            <TablePagination
                component="div"
                count={totalCount}
                page={page}
                onPageChange={handleChangePage}
                rowsPerPage={rowsPerPage}
                onRowsPerPageChange={handleChangeRowsPerPage}
            />

        </div>
    </>
}