import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
export default function Navbar() {
    const [anchorElNav, setAnchorElNav] = React.useState(null);
    const [anchorElUser, setAnchorElUser] = React.useState(null);

    const handleOpenNavMenu = (event) => {
        setAnchorElNav(event.currentTarget);
    };
    const handleOpenUserMenu = (event) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleCloseUserMenu = () => {
        setAnchorElUser(null);
    };

    return (
        <AppBar style={{position:"sticky",margin:0}}>
            <Container maxWidth="xl">
                <Toolbar disableGutters>

                    <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <MenuIcon />
                        </IconButton>
                        <Menu
                            id="menu-appbar"
                            anchorEl={anchorElNav}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'left',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'left',
                            }}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                            sx={{
                                display: { xs: 'block', md: 'none' },
                            }}
                        >
                            <MenuItem key="certificates" onClick={handleCloseNavMenu}>
                                <Typography textAlign="center">Certificates</Typography>
                            </MenuItem>
                            <MenuItem key="requests" onClick={handleCloseNavMenu}>
                                <Typography textAlign="center">Requests</Typography>
                            </MenuItem>
                            <MenuItem key="generate" onClick={handleCloseNavMenu}>
                                <Typography textAlign="center">Create Certificates</Typography>
                            </MenuItem>
                            <MenuItem key="validity" onClick={handleCloseNavMenu}>
                                <Typography textAlign="center">Check Validity</Typography>
                            </MenuItem>
                            <MenuItem key="validity" onClick={handleCloseNavMenu}>
                                <Typography textAlign="center">Logout</Typography>
                            </MenuItem>
                        </Menu>
                    </Box>
                    <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                            <Button
                                key="certificates"
                                onClick={handleCloseNavMenu}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                Certificates
                            </Button>
                        <Button
                            key="requests"
                            onClick={handleCloseNavMenu}
                            sx={{ my: 2, color: 'white', display: 'block' }}
                        >
                            Requests
                        </Button>
                        <Button
                            key="generate"
                            onClick={handleCloseNavMenu}
                            sx={{ my: 2, color: 'white', display: 'block' }}
                        >
                            Create Certificate
                        </Button>
                        <Button
                            key="validity"
                            onClick={handleCloseNavMenu}
                            sx={{ my: 2, color: 'white', display: 'block' }}
                        >
                            Check Validity
                        </Button>
                    </Box>

                    <Box sx={{ flexGrow: 0,display: { xs: 'none', md: 'flex' }  }}>
                        <Button
                            key="validity"
                            onClick={handleCloseNavMenu}
                            sx={{ my: 2, color: 'white', display: 'block' }}
                        >
                            Logout
                        </Button>
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}