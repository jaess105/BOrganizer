import { type SharedData } from '@/types';
import { Head, Link, usePage } from '@inertiajs/react';

export default function Welcome() {
    const { auth } = usePage<SharedData>().props;

    return (
        <>
            <Head title="Welcome">
                <link rel="preconnect" href="https://fonts.bunny.net" />
                <link href="https://fonts.bunny.net/css?family=instrument-sans:400,500,600" rel="stylesheet" />
            </Head>

            <div className="flex min-h-screen flex-col items-center justify-center bg-[#FDFDFC] p-6 text-[#1b1b18] dark:bg-[#0a0a0a] dark:text-[#EDEDEC]">
                <h1 className="mb-4 text-2xl font-semibold">Welcome to the Mail Templater app</h1>
                <p className="mb-6 text-center text-sm text-[#706f6c] dark:text-[#A1A09A]">Please log in to continue to your dashboard.</p>

                <nav className="flex gap-4">
                    {auth.user ? (
                        <Link
                            href={route('dashboard')}
                            className="rounded-md bg-[#1b1b18] px-5 py-2 text-sm text-white hover:bg-black dark:bg-[#eeeeec] dark:text-[#1C1C1A] dark:hover:bg-white"
                        >
                            Go to Dashboard
                        </Link>
                    ) : (
                        <>
                            <a
                                href={route('login')}
                                className="rounded-md border border-[#19140035] px-5 py-2 text-sm hover:bg-[#f0f0f0] dark:border-[#3E3E3A] dark:hover:bg-[#1C1C1A]"
                            >
                                Log In
                            </a>
                        </>
                    )}
                </nav>
            </div>
        </>
    );
}
