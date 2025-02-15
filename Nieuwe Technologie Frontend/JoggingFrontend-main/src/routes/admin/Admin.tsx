import AdminNavBar from '@/components/nav/AdminNavBar';
import { Outlet } from 'react-router-dom';

const Admin: React.FC = () => {
	return (
		<div className='flex flex-col w-full min-h-screen'>
			<AdminNavBar />
			<main className='flex min-h-[calc(100vh_-_theme(spacing.16))] flex-1 flex-col gap-4 bg-muted/40 p-4 md:gap-8 md:p-10'>
				<Outlet />
			</main>
		</div>
	);
};

export default Admin;
