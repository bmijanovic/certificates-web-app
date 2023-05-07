import axios from "axios";
import {Box, FormControl, Grid, InputLabel, MenuItem, Select, Tab, TablePagination, Tabs} from "@mui/material";
import {TabContext} from "@mui/lab";
import React, {useContext, useEffect, useState} from "react";
import CertificateCard from "../components/CertificateCard.jsx";

export default function AllCertificates(){
    const [value, setValue] = useState(0);
    const [page, setPage] = React.useState(0);
    const [totalCount, setTotalCount] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(10);
    const [certificates,setCertificates]=React.useState([]);
    const handleChangePage = (
        event, newPage) => {
        setPage(newPage);
    };
    const handleChangeRowsPerPage = (
        event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };
    useEffect( ()=>{
        switch (value) {
            case 0:
                axios.get(`https://localhost:7018/api/Certificate?PageNumber=${page + 1}&PageSize=${rowsPerPage}`).then(res => {
                    setTotalCount(res.data.totalCount);
                    setCertificates(res.data.certificates)
                }).catch(err => {
                    console.log(err)
                });
                break;
            case 1:
                axios.get(`https://localhost:7018/api/Certificate/my?PageNumber=${page + 1}&PageSize=${rowsPerPage}`).then(res => {
                    setTotalCount(res.data.totalCount);
                    setCertificates(res.data.certificates)
                }).catch(err => {
                    console.log(err)
                });
                break;
        }
    },[page,rowsPerPage,value])
    const handleChange = (event, newValue) => {
        setValue(newValue.props.value);
        setPage(0);
    };
    const renderPanel = () => {
        return <>
            {certificates.length===0 ? <p>No certificates to show...</p> : <div>
                <Grid container sx={{bx:3, mt:1}} columnSpacing={25}>
                    {certificates.map(item => <CertificateCard key={item.serialNumber} data={item} acceptable={false}/>)}
                </Grid>
            </div>
            }
        </>
    };


    return <>
        <div style={{textAlign: "center", width: "80%", margin:"auto"}}>
            <h1>Certificates</h1>
        <div style={{display:"flex", justifyContent:"end", width:"100%"}}>
            <FormControl >
                <InputLabel id="demo-simple-select-label">Certificates</InputLabel>
                <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    value={value}
                    label="Certificates"
                    onChange={handleChange}>
                    <MenuItem value={0}>All Certificates</MenuItem>
                    <MenuItem value={1}>My Certificates</MenuItem>
                </Select>
            </FormControl>
        </div>
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