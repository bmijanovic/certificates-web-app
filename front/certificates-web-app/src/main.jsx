import React from 'react'
import ReactDOM from 'react-dom/client'
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import GenerateCertificateRequest from "./pages/GenerateCertificateRequest.jsx";
import "./index.css"
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import AllCertificateRequests from "./pages/AllCertificateRequests.jsx";
import {ReactQueryDevtools} from "@tanstack/react-query-devtools";

const queryClient = new QueryClient({defaultOptions: { queries: {
            staleTime: 1000 * 60 * 2
        }}})

const router = createBrowserRouter([
    {path:"/generate", element: <GenerateCertificateRequest/>},
    {path:"/requests", element: <AllCertificateRequests/>},
])

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router}/>
        <ReactQueryDevtools/>
      </QueryClientProvider>
  </React.StrictMode>,
)
